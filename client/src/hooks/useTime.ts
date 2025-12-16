import { useState, useEffect, useCallback } from 'react';

const API_BASE_URL = 'http://localhost:5000/api';

interface UseTimeResult {
  utcTime: Date;
  isLoading: boolean;
  error: string | null;
  refetch: () => void;
}

export function useTime(): UseTimeResult {
  const [utcTime, setUtcTime] = useState<Date>(new Date());
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [serverOffset, setServerOffset] = useState<number>(0);

  const fetchUtcTime = useCallback(async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/time/utc`);
      if (!response.ok) {
        throw new Error('Failed to fetch UTC time');
      }
      const data = await response.json();
      const serverTime = new Date(data.utcTime);
      const localTime = new Date();

      // Calculate offset between server time and local time
      setServerOffset(serverTime.getTime() - localTime.getTime());
      setUtcTime(serverTime);
      setError(null);
    } catch (err) {
      // Fallback to local time if server is unavailable
      setUtcTime(new Date());
      setServerOffset(0);
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchUtcTime();
  }, [fetchUtcTime]);

  // Update time every second using the calculated offset
  useEffect(() => {
    const interval = setInterval(() => {
      const now = new Date();
      setUtcTime(new Date(now.getTime() + serverOffset));
    }, 1000);

    return () => clearInterval(interval);
  }, [serverOffset]);

  return { utcTime, isLoading, error, refetch: fetchUtcTime };
}
