import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Article from '../index';

describe('<Article />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Article />);
  });
});
