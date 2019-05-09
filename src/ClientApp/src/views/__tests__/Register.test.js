import React from 'react';
import { render, cleanup, fireEvent, wait } from 'react-testing-library';
import { MemoryRouter } from 'react-router';
import mockAxios from 'jest-mock-axios';
import faker from 'faker';
import { Provider } from 'outstated';
import userStore from '../../stores/userStore';
import Register from '../Register';

describe('<Register />', () => {
  afterEach(() => {
    mockAxios.reset();
    cleanup();
  });

  function renderRegister() {
    return render(
      <Provider stores={[userStore]}>
        <MemoryRouter>
          <Register />
        </MemoryRouter>
      </Provider>
    );
  }

  it('should render without crashing', () => {
    renderRegister();
  });

  it('should send HTTP request when the form is submitted with valid values', async () => {
    // Arrange
    const username = faker.internet.userName();
    const email = faker.internet.email();
    const password = faker.internet.password();

    // Act
    const { container } = renderRegister();

    fireEvent.change(container.querySelector('[name="username"]'), { target: { value: username } });
    fireEvent.change(container.querySelector('[name="email"]'), { target: { value: email } });
    fireEvent.change(container.querySelector('[type="password"]'), { target: { value: password } });
    fireEvent.submit(container.querySelector('form'));

    // Assert
    await wait(() => {
      expect(mockAxios.post).toHaveBeenCalledTimes(1);
    });
  });
});
