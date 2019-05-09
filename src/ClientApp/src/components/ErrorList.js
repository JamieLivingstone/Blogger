import React from 'react';
import PropTypes from 'prop-types';

function ErrorList({ errors }) {
  return (
    <ul className="validation-errors">
      {errors.map(error => (
        <li key={error}>{error}</li>
      ))}
    </ul>
  );
}

ErrorList.propTypes = {
  errors: PropTypes.arrayOf(PropTypes.string).isRequired
};
export default ErrorList;
