import React from 'react';
import PropTypes from 'prop-types';

function Banner(props) {
  return (
    <div className="banner" style={{ backgroundColor: props.bgColor, color: props.color }}>
      <div className="container text-center">{props.children}</div>
    </div>
  );
}

Banner.defaultProps = {
  color: '#FFFFFF'
};

Banner.propTypes = {
  color: PropTypes.string,
  bgColor: PropTypes.string.isRequired,
  children: PropTypes.node.isRequired
};

export default Banner;
