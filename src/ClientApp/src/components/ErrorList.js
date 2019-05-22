import React from 'react';
import PropTypes from 'prop-types';

function ErrorList({ errors }) {
  let formattedErrors = errors;

  if (!Array.isArray(errors) && errors !== null && typeof errors === 'object') {
    formattedErrors = Object.keys(errors)
      .map(key => errors[key])
      .flat();
  }

  return (
    <ul className="validation-errors">
      {formattedErrors.map(error => (
        <li key={error}>{error}</li>
      ))}
    </ul>
  );
}

ErrorList.propTypes = {
  errors: PropTypes.oneOfType([PropTypes.array, PropTypes.object])
};

export default ErrorList;
