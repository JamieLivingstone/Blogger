import React from 'react';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';

const loggedOutNavigation = () => (
  <nav>
    <Link to="/">Home</Link>
    <Link to="/login">Sign In</Link>
    <Link to="/register">Sign up</Link>
  </nav>
);

export const loggedInNavigation = props => {
  const username = props.currentUser.username;

  return (
    <nav>
      <Link to="/">Home</Link>
      <Link to="/">New Article</Link>
      <Link to="/">Settings</Link>
      <Link to={`/@${username}`}>{username}</Link>
    </nav>
  );
};

function Header(props) {
  const Navigation = props.currentUser ? loggedInNavigation : loggedOutNavigation;

  return (
    <header>
      <Navigation {...props} />
    </header>
  );
}

Header.propTypes = {
  currentUser: PropTypes.object
};

export default Header;
