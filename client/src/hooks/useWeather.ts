import { useState, useCallback, useRef } from 'react';
import type { Weather } from '../types';

const API_BASE_URL = 'http://localhost:5000/api';

interface UseWeatherResult {
  weather: Weather | null;
  isLoading: boolean;
  error: string | null;
  fetchWeather: (lat: number, lon: number, forceRefresh?: boolean) => void;
}

// Simple cache for weather data
const weatherCache = new Map<string, { data: Weather; timestamp: number }>();
const CACHE_DURATION = 5 * 60 * 1000; // 5 minutes

export function useWeather(): UseWeatherResult {
  const [weather, setWeather] = useState<Weather | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const currentRequestRef = useRef<string | null>(null);

  const fetchWeather = useCallback(async (lat: number, lon: number, forceRefresh = false) => {
    const cacheKey = `${lat},${lon}`;

    // Track current request to prevent race conditions
    currentRequestRef.current = cacheKey;

    // Check cache first (unless force refresh)
    if (!forceRefresh) {
      const cached = weatherCache.get(cacheKey);
      if (cached && Date.now() - cached.timestamp < CACHE_DURATION) {
        setWeather(cached.data);
        return;
      }
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await fetch(
        `${API_BASE_URL}/weather?lat=${lat}&lon=${lon}`
      );
      if (!response.ok) {
        throw new Error('Failed to fetch weather');
      }
      const data: Weather = await response.json();

      // Only update state if this is still the current request
      if (currentRequestRef.current === cacheKey) {
        setWeather(data);
        // Cache the result
        weatherCache.set(cacheKey, { data, timestamp: Date.now() });
      }
    } catch (err) {
      if (currentRequestRef.current === cacheKey) {
        setError(err instanceof Error ? err.message : 'Unknown error');
      }
    } finally {
      if (currentRequestRef.current === cacheKey) {
        setIsLoading(false);
      }
    }
  }, []);

  return { weather, isLoading, error, fetchWeather };
}
