import { renderHook, act } from 'react-hooks-testing-library';
import axios from 'axios';
import mockAxios from 'jest-mock-axios';
import userStore from '../userStore';

describe('User Store', () => {
  afterEach(() => {
    mockAxios.reset();
  });

  it('should expose public API', () => {
    // Act
    const { result } = renderHook(() => userStore());

    // Assert
    expect(result.current.user).toBeNull();
    expect(result.current.login).toBeInstanceOf(Function);
    expect(result.current.register).toBeInstanceOf(Function);
  });

  it('should set user and mutate axios bearer token on successful login', async () => {
    // Arrange
    const responseObj = {
      data: {
        userName: 'test',
        token: 'secret token here...'
      }
    };

    // Act
    const { result } = renderHook(() => userStore());

    act(() => {
      result.current.login('fake', 'password');
      mockAxios.mockResponse(responseObj);
    });

    // Assert
    expect(mockAxios.post).toHaveBeenCalledTimes(1);
    expect(result.current.user.userName).toEqual(responseObj.data.userName);
    expect(axios.defaults.headers.common.Authorization).toContain(responseObj.data.token);
  });
});
