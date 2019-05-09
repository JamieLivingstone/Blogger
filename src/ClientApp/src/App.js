import React, { Suspense } from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';
import Header from './components/Header';
import { Provider } from 'outstated';

// Stores
import userStore from './stores/userStore';

// Views
const Feed = React.lazy(() => import('./views/Feed'));
const Login = React.lazy(() => import('./views/Login'));
const Register = React.lazy(() => import('./views/Register'));
const Editor = React.lazy(() => import('./views/Editor'));
const Article = React.lazy(() => import('./views/Article'));
const Profile = React.lazy(() => import('./views/Profile'));
const Settings = React.lazy(() => import('./views/Settings'));

const App = () => (
  <div>
    <Provider stores={[userStore]}>
      <Router>
        <Header />

        <Suspense fallback={<div />}>
          <Route exact path="/" component={Feed} />
          <Route exact path="/login" component={Login} />
          <Route exact path="/register" component={Register} />
          <Route exact path="/editor" component={Editor} />
          <Route exact path="/editor/:slug" component={Editor} />
          <Route exact path="/article/:id" component={Article} />
          <Route exact path="/@:profile" component={Profile} />
          <Route exact path="/settings" component={Settings} />
        </Suspense>
      </Router>
    </Provider>
  </div>
);

export default App;
