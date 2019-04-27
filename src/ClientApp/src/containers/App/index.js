import React, { Suspense } from 'react';
import { BrowserRouter as Router, Route } from 'react-router-dom';

const Header = React.lazy(() => import('../../components/Header'));
const Footer = React.lazy(() => import('../../components/Footer'));
const Home = React.lazy(() => import('../Home'));
const Login = React.lazy(() => import('../Login'));
const Register = React.lazy(() => import('../Register'));
const Editor = React.lazy(() => import('../Editor'));
const Article = React.lazy(() => import('../Article'));
const Profile = React.lazy(() => import('../Profile'));

const App = () => (
  <div>
    <Suspense fallback={<div>Loading...</div>}>
      <Router>
        <Header />

        <Route exact path="/" component={Home} />
        <Route exact path="/login" component={Login} />
        <Route exact path="/register" component={Register} />
        <Route exact path="/editor" component={Editor} />
        <Route exact path="/editor/:slug" component={Editor} />
        <Route exact path="/article/:id" component={Article} />
        <Route exact path="/@:profile" component={Profile} />

        <Footer />
      </Router>
    </Suspense>
  </div>
);

export default App;
