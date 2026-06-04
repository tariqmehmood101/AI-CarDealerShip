import { render, screen } from '@testing-library/react';
import Home from '../pages/index';

describe('Home page', () => {
  const defaultFetch = global.fetch;

  afterEach(() => {
    global.fetch = defaultFetch;
  });

  it('renders the landing page', async () => {
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () => Promise.resolve({ status: 'Healthy', application: 'CarDealership DMV Automation API' })
      }) as unknown
    ) as any;

    render(<Home />);

    expect(await screen.findByRole('heading', { name: /Car Dealership DMV Automation/i })).toBeInTheDocument();
    expect(await screen.findByText(/backend status:/i)).toBeInTheDocument();
    expect(screen.getByText(/backend health:/i)).toBeInTheDocument();
  });

  it('displays an error when the backend health request fails', async () => {
    global.fetch = jest.fn(() => Promise.reject(new Error('Network error'))) as any;

    render(<Home />);

    expect(await screen.findByText(/unable to reach backend health/i)).toBeInTheDocument();
  });

  it('displays an error when the backend returns an error status', async () => {
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: false,
        status: 500,
        json: () => Promise.reject(new Error('JSON parse error'))
      }) as unknown
    ) as any;

    render(<Home />);

    expect(await screen.findByText(/unable to reach backend health/i)).toBeInTheDocument();
  });
});
