import { useEffect, useState } from 'react';

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5000';

export default function Home() {
  const [healthMessage, setHealthMessage] = useState<string>('Loading backend status...');
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function checkHealth() {
      try {
        const response = await fetch(`${apiBaseUrl}/health`);
        if (!response.ok) {
          throw new Error(`Status ${response.status}`);
        }
        const data = await response.json();
        setHealthMessage(`Backend status: ${data.status} (${data.application})`);
      } catch (error) {
        setHealthMessage(`Unable to reach backend health at ${apiBaseUrl}/health`);
      } finally {
        setIsLoading(false);
      }
    }

    checkHealth();
  }, []);

  return (
    <main style={{ maxWidth: 760, margin: '0 auto', padding: '2rem', fontFamily: 'system-ui, sans-serif' }}>
      <h1>Car Dealership DMV Automation</h1>
      <p>Minimal MVP landing page for the Car Dealership SaaS monorepo.</p>
      <p>
        Frontend expects the backend health endpoint at <code>{apiBaseUrl}/health</code>.
      </p>
      <div style={{ padding: '1rem', border: '1px solid #ddd', borderRadius: 8, marginTop: '1rem' }}>
        <strong>Backend health:</strong>
        <p>{isLoading ? 'Checking...' : healthMessage}</p>
        <a href={`${apiBaseUrl}/health`} target="_blank" rel="noopener noreferrer">
          Open API health endpoint
        </a>
      </div>
    </main>
  );
}
