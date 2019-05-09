import React from 'react';
import { render, cleanup } from 'react-testing-library';
import ErrorList from '../ErrorList';

describe('<ErrorList />', () => {
  afterEach(cleanup);

  it('should render without crashing', () => {
    render(<ErrorList errors={[]} />);
  });

  it('should render errors into list', () => {
    // Arrange
    const errors = ['UserName is already taken', 'Password too weak'];

    // Act
    const { container, getByText } = render(<ErrorList errors={errors} />);

    // Assert
    expect(container.querySelectorAll('li').length).toEqual(errors.length);
    errors.forEach(getByText);
  });
});
