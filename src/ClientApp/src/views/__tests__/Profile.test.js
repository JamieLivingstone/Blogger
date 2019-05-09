import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Profile from '../Profile';

describe('<Profile />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Profile />);
  });
});
