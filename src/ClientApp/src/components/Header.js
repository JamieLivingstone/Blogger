import React from 'react';
import { NavLink } from 'react-router-dom';
import { useStore } from 'outstated';
import userStore from '../stores/userStore';

const loggedOutNavigation = () => (
  <nav>
    <NavLink to="/" activeClassName="active" exact>
      Home
    </NavLink>

    <NavLink to="/login" activeClassName="active" exact>
      Sign In
    </NavLink>

    <NavLink to="/register" activeClassName="active" exact>
      Sign up
    </NavLink>
  </nav>
);

const loggedInNavigation = () => {
  const { user } = useStore(userStore);
  const username = user.userName;

  return (
    <nav>
      <NavLink to="/" activeClassName="active" exact>
        Home
      </NavLink>

      <NavLink to="/editor" activeClassName="active" exact>
        New Article
      </NavLink>

      <NavLink to="/settings" activeClassName="active" exact>
        Settings
      </NavLink>

      <NavLink to={`/@${username}`} activeClassName="active" exact>
        {username}
      </NavLink>
    </nav>
  );
};

function HeaderComponent() {
  const { user } = useStore(userStore);
  const Navigation = user ? loggedInNavigation : loggedOutNavigation;

  return (
    <div className="container">
      <header>
        <NavLink to="/" className="logo">
          blogger
        </NavLink>

        <Navigation />
      </header>
    </div>
  );
}

export default HeaderComponent;
