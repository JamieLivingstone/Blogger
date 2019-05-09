import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Feed from '../Feed';

describe('<Feed />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Feed />);
  });
});
