import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Editor from '../Editor';

describe('<Editor />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Editor />);
  });
});
