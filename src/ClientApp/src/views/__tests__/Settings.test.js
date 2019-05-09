import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Settings from '../Settings';

describe('<Settings />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Settings />);
  });
});
