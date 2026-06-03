import { render, screen } from '@testing-library/react';
import Home from '../pages/index';

describe('Home page', () => {
  beforeAll(() => {
    global.fetch = jest.fn(() =>
      Promise.resolve({
        ok: true,
        json: () => Promise.resolve({ status: 'Healthy', application: 'CarDealership DMV Automation API' })
      }) as unknown
    ) as any;
  });

  it('renders the landing page', async () => {
    render(<Home />);

    expect(await screen.findByRole('heading', { name: /Car Dealership DMV Automation/i })).toBeInTheDocument();
    expect(screen.getByText(/backend health:/i)).toBeInTheDocument();
  });
});
