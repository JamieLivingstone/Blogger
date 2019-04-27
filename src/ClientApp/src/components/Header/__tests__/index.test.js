import React from 'react';
import { render, cleanup } from 'react-testing-library';
import { MemoryRouter } from 'react-router';
import Header from '../index';

describe('<Header />', () => {
  afterEach(cleanup);

  function renderHeader(props) {
    return render(
      <MemoryRouter>
        <Header {...props} />
      </MemoryRouter>
    );
  }

  it('should render without crashing', () => {
    renderHeader();
  });

  it('should render logged out navigation menu if a user is not signed in', () => {
    // Arrange
    const props = { currentUser: null };

    // Act
    const { getByText } = renderHeader(props);

    // Assert
    expect(getByText('Sign In').nodeName).toEqual('A');
  });

  it('should render logged in navigation menu if a user is signed in', () => {
    // Arrange
    const props = { currentUser: { username: 'Test' } };

    // Act
    const { getByText } = renderHeader(props);

    // Assert
    expect(getByText(props.currentUser.username).nodeName).toEqual('A');
  });
});
