import React from 'react';
import { render, cleanup } from 'react-testing-library';
import { MemoryRouter } from 'react-router';
import { Provider } from 'outstated';
import userStore from '../../stores/userStore';
import Header from '../Header';

describe('<Header />', () => {
  afterEach(cleanup);

  function renderHeader() {
    return render(
      <Provider stores={[userStore]}>
        <MemoryRouter>
          <Header />
        </MemoryRouter>
      </Provider>
    );
  }

  it('should render without crashing', () => {
    renderHeader();
  });

  it('should render logged out navigation menu if a user is not signed in', () => {
    // Act
    const { getByText } = renderHeader();

    // Assert
    expect(getByText('Sign In').nodeName).toEqual('A');
  });
});
