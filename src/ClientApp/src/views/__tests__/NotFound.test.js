import React from 'react';
import { render, cleanup } from 'react-testing-library';
import NotFound from '../NotFound';

describe('<NotFound />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<NotFound />);
  });
});
