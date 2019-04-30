import React from 'react';
import { NavLink } from 'react-router-dom';
import PropTypes from 'prop-types';
import Container from '../Layout/Container';
import Logo from '../Logo';
import { Header, Nav } from './styles';

const loggedOutNavigation = () => (
  <Nav>
    <NavLink to="/" exact>
      Home
    </NavLink>

    <NavLink to="/login" exact>
      Sign In
    </NavLink>

    <NavLink to="/register" exact>
      Sign up
    </NavLink>
  </Nav>
);

const loggedInNavigation = props => {
  const username = props.currentUser.username;

  return (
    <Nav>
      <NavLink to="/" exact>
        Home
      </NavLink>

      <NavLink to="/" exact>
        New Article
      </NavLink>

      <NavLink to="/" exact>
        Settings
      </NavLink>

      <NavLink to={`/@${username}`} exact>
        {username}
      </NavLink>
    </Nav>
  );
};

function HeaderComponent(props) {
  const Navigation = props.currentUser ? loggedInNavigation : loggedOutNavigation;

  return (
    <Container>
      <Header>
        <Logo />
        <Navigation {...props} />
      </Header>
    </Container>
  );
}

HeaderComponent.propTypes = {
  currentUser: PropTypes.object
};

export default HeaderComponent;
