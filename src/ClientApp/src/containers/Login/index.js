import React, { Fragment } from 'react';
import axios from 'axios';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';

const LoginSchema = Yup.object().shape({
  username: Yup.string().required(),
  password: Yup.string().required()
});

function Login() {
  function submit(values) {
    axios.post('http://localhost:5000/api/users/login', values).then(response => {
      console.log(response);
    });
  }

  return (
    <Fragment>
      <h1>Sign in</h1>

      <Formik
        initialValues={{ username: '', password: '' }}
        validationSchema={LoginSchema}
        onSubmit={submit}
      >
        {({ errors, touched }) => (
          <Form>
            <Field name="username" />
            {errors.username && touched.username ? <div>{errors.username}</div> : null}

            <Field name="password" type="password" />
            {errors.password && touched.password ? <div>{errors.password}</div> : null}

            <button type="submit">Sign in</button>
          </Form>
        )}
      </Formik>
    </Fragment>
  );
}

export default Login;
