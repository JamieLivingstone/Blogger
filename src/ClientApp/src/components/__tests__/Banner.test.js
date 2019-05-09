import React from 'react';
import { render, cleanup } from 'react-testing-library';
import Banner from '../Banner';

describe('<Banner />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Banner bgColor="#000">test</Banner>);
  });

  it('should render children', () => {
    // Arrange
    const title = 'Example!';

    // Act
    const { getByText } = render(
      <Banner bgColor="#000">
        <h1>{title}</h1>
      </Banner>
    );

    // Assert
    expect(getByText(title).nodeName).toEqual('H1');
  });
});
