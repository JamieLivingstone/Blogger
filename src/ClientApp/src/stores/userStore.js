import { useState, useEffect } from 'react';
import axios from 'axios';

const JWT_STORAGE_KEY = 'jwt_token';

function userStore() {
  const [user, setUser] = useState(null);

  useEffect(function tokenBasedLogin() {
    const token = window.localStorage.getItem(JWT_STORAGE_KEY);

    if (token) {
      axios
        .get(`${process.env.API_URL}/api/users`, {
          headers: { Authorization: `Bearer ${token}` }
        })
        .then(handleSuccessfulAuthentication)
        .catch(() => {
          window.localStorage.removeItem(JWT_STORAGE_KEY);
        });
    }
  }, []);

  function login(userName, password) {
    return new Promise((resolve, reject) => {
      axios
        .post(`${process.env.API_URL}/api/users/login`, { username: userName, password })
        .then(response => {
          handleSuccessfulAuthentication(response);
          resolve();
        })
        .catch(() => {
          reject(['Login failed.']);
        });
    });
  }

  function register(user) {
    return new Promise((resolve, reject) => {
      axios
        .post(`${process.env.API_URL}/api/users`, user)
        .then(response => {
          handleSuccessfulAuthentication(response);
          resolve();
        })
        .catch(({ response }) => {
          const errors = response.data.map(error => error.description);
          reject(errors);
        });
    });
  }

  function handleSuccessfulAuthentication({ data }) {
    const { token, ...user } = data;
    setUser(user);
    window.localStorage.setItem(JWT_STORAGE_KEY, token);
    axios.defaults.headers.common.Authorization = `Bearer ${token}`;
  }

  return { user, login, register };
}

export default userStore;
