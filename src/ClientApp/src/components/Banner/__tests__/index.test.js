import React from 'react';
import { render, cleanup } from 'react-testing-library';
import 'jest-styled-components';
import Banner from '../index';

describe('<Banner />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<Banner>test</Banner>);
  });

  it('should render children', () => {
    // Arrange
    const title = 'Example!';

    // Act
    const { getByText } = render(
      <Banner>
        <h1>{title}</h1>
      </Banner>
    );

    // Assert
    expect(getByText(title).nodeName).toEqual('H1');
  });

  it('should apply colors from props', () => {
    // Arrange
    const background = '#000';
    const color = '#FFF';

    // Act
    const { container } = render(
      <Banner background={background} color={color}>
        test
      </Banner>
    );

    // Assert
    expect(container.firstChild).toHaveStyleRule('background', background);
    expect(container.firstChild).toHaveStyleRule('color', color);
  });
});
