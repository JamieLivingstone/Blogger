import React, { useState } from 'react';
import { Formik } from 'formik';
import { Link, Redirect } from 'react-router-dom';
import { useStore } from 'outstated';
import userStore from '../stores/userStore';
import ErrorList from '../components/ErrorList';

function Login() {
  const store = useStore(userStore);
  const [errors, setErrors] = useState([]);

  async function submit(values, { setSubmitting }) {
    await store.login(values.username, values.password).catch(() => {
      setErrors(['Login failed.']);
    });

    setSubmitting(false);
  }

  if (store.user) {
    return <Redirect to="/" />;
  }

  return (
    <div className="page container">
      <div className="text-center">
        <h1>Sign in</h1>
        <Link to="/register">Need an account?</Link>
      </div>

      <Formik initialValues={{ username: '', password: '' }} onSubmit={submit}>
        {({ handleSubmit, handleChange, isSubmitting }) => (
          <form onSubmit={handleSubmit} className={isSubmitting ? 'submitting' : null}>
            <ErrorList errors={errors} />

            <input
              type="text"
              required
              name="username"
              placeholder="Username"
              onChange={handleChange}
            />

            <input
              type="password"
              required
              name="password"
              placeholder="Password"
              onChange={handleChange}
            />

            <button type="submit">Sign in</button>
          </form>
        )}
      </Formik>
    </div>
  );
}

export default Login;
