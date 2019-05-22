import React, { useState } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';
import { Formik } from 'formik';
import { withRouter } from 'react-router-dom';
import ErrorList from '../components/ErrorList';

function Editor(props) {
  const [errors, setErrors] = useState([]);

  async function submit(values, { setSubmitting }) {
    await axios
      .post(`${process.env.API_URL}/api/articles`, {
        ...values,
        tags: values.tags.split(',').map(x => x.trim())
      })
      .then(({ data }) => {
        props.history.push(`/article/${data.slug}`);
      });

    setSubmitting(false);
  }

  return (
    <div className="page container">
      <Formik initialValues={{ title: '', description: '', body: '', tags: '' }} onSubmit={submit}>
        {({ handleSubmit, handleChange, isSubmitting }) => (
          <form onSubmit={handleSubmit} className={isSubmitting ? 'submitting' : null}>
            <ErrorList errors={errors} />

            <input
              type="text"
              name="title"
              placeholder="Article Title"
              onChange={handleChange}
            />

            <input
              type="text"
              name="description"
              placeholder="What's this article about?"
              onChange={handleChange}
              minLength={5}
            />

            <textarea
              name="body"
              placeholder="Write your article (in markdown)"
              rows={8}
              onChange={handleChange}
              minLength={10}
            />

            <input type="text" name="tags" placeholder="Enter tags" onChange={handleChange} />

            <button type="submit">Publish Article</button>
          </form>
        )}
      </Formik>
    </div>
  );
}

Editor.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func.isRequired
  })
};

export default withRouter(Editor);
