import "@testing-library/jest-dom";

jest.mock("axios", () => {
  const mockAxiosInstance = {
    get: jest.fn(),
    post: jest.fn(),
    delete: jest.fn(),
    interceptors: {
      request: { use: jest.fn() },
      response: { use: jest.fn() }
    },
  };

  return {
    create: () => mockAxiosInstance,
  };
});
