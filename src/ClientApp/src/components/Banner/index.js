import React from 'react';
import PropTypes from 'prop-types';
import { BannerWrapper } from './styles';

function Banner(props) {
  return <BannerWrapper {...props}>{props.children}</BannerWrapper>;
}

Banner.defaultProps = {
  color: '#FFF',
  background: '#5cb85c',
  center: false
};

Banner.propTypes = {
  color: PropTypes.string,
  background: PropTypes.string,
  children: PropTypes.node.isRequired,
  center: PropTypes.bool
};

export default Banner;
