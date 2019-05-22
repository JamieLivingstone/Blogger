import React from 'react';
import { render, cleanup, fireEvent, wait } from 'react-testing-library';
import { MemoryRouter } from 'react-router';
import mockAxios from 'jest-mock-axios';
import Editor from '../Editor';

describe('<Editor />', () => {
  afterEach(() => {
    mockAxios.reset();
    cleanup();
  });

  function renderEditor() {
    return render(
      <MemoryRouter>
        <Editor />
      </MemoryRouter>
    );
  }

  it('should render without crashing', () => {
    renderEditor();
  });

  it('should create article', async () => {
    // Act
    const { container } = renderEditor();

    fireEvent.change(container.querySelector('[name="title"]'), {
      target: { value: 'My article title!' }
    });

    fireEvent.change(container.querySelector('[name="description"]'), {
      target: { value: 'Description goes here!' }
    });

    fireEvent.change(container.querySelector('[name="body"]'), {
      target: { value: 'Body goes here!' }
    });

    fireEvent.submit(container.querySelector('form'));

    // Assert
    await wait(() => {
      expect(mockAxios.post).toHaveBeenCalledTimes(1);
    });
  });
});
