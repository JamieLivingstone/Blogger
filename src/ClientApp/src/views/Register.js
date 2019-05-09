import React, { useState } from 'react';
import { Formik } from 'formik';
import { Link, Redirect } from 'react-router-dom';
import { useStore } from 'outstated';
import userStore from '../stores/userStore';
import ErrorList from '../components/ErrorList';

function Register() {
  const store = useStore(userStore);
  const [errors, setErrors] = useState([]);

  async function submit(values, { setSubmitting }) {
    await store.register(values).catch(setErrors);
    setSubmitting(false);
  }

  // Redirect to home page on successful login or if user is already signed in
  if (store.user) {
    return <Redirect to="/" />;
  }

  return (
    <div className="page container">
      <div className="text-center">
        <h1>Sign up</h1>
        <Link to="/login">Have an account?</Link>
      </div>

      <Formik initialValues={{ username: '', email: '', password: '' }} onSubmit={submit}>
        {({ handleSubmit, handleChange, isSubmitting }) => (
          <form onSubmit={handleSubmit} className={isSubmitting ? 'submitting' : null}>
            <ErrorList errors={errors} />

            <input type="text" name="username" placeholder="Username" onChange={handleChange} />
            <input type="email" name="email" placeholder="Email" onChange={handleChange} />
            <input type="password" name="password" placeholder="Password" onChange={handleChange} />
            <button type="submit">Sign up</button>
          </form>
        )}
      </Formik>
    </div>
  );
}

export default Register;
