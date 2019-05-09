import React from 'react';
import { render, cleanup, fireEvent, wait } from 'react-testing-library';
import { MemoryRouter } from 'react-router';
import mockAxios from 'jest-mock-axios';
import faker from 'faker';
import { Provider } from 'outstated';
import userStore from '../../stores/userStore';
import Login from '../Login';

describe('<Login />', () => {
  afterEach(() => {
    mockAxios.reset();
    cleanup();
  });

  function renderLogin() {
    return render(
      <Provider stores={[userStore]}>
        <MemoryRouter>
          <Login />
        </MemoryRouter>
      </Provider>
    );
  }

  it('should render without crashing', () => {
    renderLogin();
  });

  it('should send HTTP request when the form is submitted with valid values', async () => {
    // Arrange
    const username = faker.internet.userName();
    const password = faker.internet.password();

    // Act
    const { container } = renderLogin();

    fireEvent.change(container.querySelector('[name="username"]'), { target: { value: username } });
    fireEvent.change(container.querySelector('[type="password"]'), { target: { value: password } });
    fireEvent.submit(container.querySelector('form'));

    // Assert
    await wait(() => {
      expect(mockAxios.post).toHaveBeenCalledTimes(1);
    });
  });
});
