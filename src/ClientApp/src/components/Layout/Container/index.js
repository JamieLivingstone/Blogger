import React from 'react';
import PropTypes from 'prop-types';
import { ContainerWrapper } from './styles';

function Container(props) {
  return <ContainerWrapper>{props.children}</ContainerWrapper>;
}

Container.propTypes = {
  children: PropTypes.node.isRequired
};

export default Container;
