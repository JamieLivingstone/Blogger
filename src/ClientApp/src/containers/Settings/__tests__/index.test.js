import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Settings from '../index';

describe('<Settings />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Settings />);
  });
});
