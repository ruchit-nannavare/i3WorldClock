import { useState, useCallback, useRef, useEffect } from 'react';
import type { City } from '../types';
import { useLocalStorage } from './useLocalStorage';

const API_BASE_URL = 'http://localhost:5000/api';
console.log('🌐 API_BASE_URL configured as:', API_BASE_URL);

interface UseCitiesResult {
  savedCities: City[];
  primaryCityId: string | null;
  searchResults: City[];
  isSearching: boolean;
  searchError: string | null;
  searchCities: (query: string) => void;
  addCity: (city: City) => void;
  removeCity: (cityId: string) => void;
  setPrimaryCity: (cityId: string) => void;
  clearSearch: () => void;
}

// Default cities for initial state
const defaultCities: City[] = [
  {
    id: 'london-gb',
    name: 'London',
    country: 'United Kingdom',
    countryCode: 'GB',
    latitude: 51.5074,
    longitude: -0.1278,
    utcOffsetSeconds: 0,
    utcOffsetDisplay: 'UTC+0',
  },
];

export function useCities(): UseCitiesResult {
  const [savedCities, setSavedCities] = useLocalStorage<City[]>(
    'i3worldclock-cities',
    defaultCities
  );
  const [primaryCityId, setPrimaryCityIdState] = useLocalStorage<string | null>(
    'i3worldclock-primary-city',
    defaultCities[0]?.id ?? null
  );
  const [searchResults, setSearchResults] = useState<City[]>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);

  // Debounce timer ref
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  // Reset to defaults if savedCities is empty (handles corrupted localStorage)
  useEffect(() => {
    if (savedCities.length === 0) {
      setSavedCities(defaultCities);
      setPrimaryCityIdState(defaultCities[0]?.id ?? null);
    }
  }, [savedCities.length, setSavedCities, setPrimaryCityIdState]);

  const searchCities = useCallback((query: string) => {
    // Clear previous debounce
    if (debounceRef.current) {
      clearTimeout(debounceRef.current);
    }

    if (!query || query.length < 2) {
      setSearchResults([]);
      setSearchError(null);
      setIsSearching(false);
      return;
    }

    // Set searching state immediately to show loading indicator
    setIsSearching(true);

    // Debounce by 300ms
    debounceRef.current = setTimeout(async () => {
      setSearchError(null);

      try {
        const url = `${API_BASE_URL}/cities/search?query=${encodeURIComponent(query)}`;
        console.log('🔍 Searching cities with URL:', url);
        const response = await fetch(url);
        console.log('📡 Response status:', response.status, response.ok);
        if (!response.ok) {
          throw new Error('Failed to search cities');
        }
        const data: City[] = await response.json();
        console.log('✅ Search results:', data.length, 'cities found');
        setSearchResults(data);
      } catch (err) {
        console.error('❌ Search error:', err);
        setSearchError(err instanceof Error ? err.message : 'Unknown error');
        setSearchResults([]);
      } finally {
        setIsSearching(false);
      }
    }, 300);
  }, []);

  const addCity = useCallback(
    (city: City) => {
      setSavedCities((prev) => {
        if (prev.some((c) => c.id === city.id)) {
          // City already exists, just set it as primary
          setPrimaryCityIdState(city.id);
          return prev;
        }
        return [...prev, city];
      });
      // Set the newly added city as the primary city
      setPrimaryCityIdState(city.id);
    },
    [setSavedCities, setPrimaryCityIdState]
  );

  const removeCity = useCallback(
    (cityId: string) => {
      setSavedCities((prev) => {
        const remaining = prev.filter((c) => c.id !== cityId);

        // If removing the last city, reset to defaults
        if (remaining.length === 0) {
          setPrimaryCityIdState(defaultCities[0]?.id ?? null);
          return defaultCities;
        }

        // If removing the primary city, set a new primary
        if (primaryCityId === cityId) {
          setPrimaryCityIdState(remaining[0].id);
        }

        return remaining;
      });
    },
    [primaryCityId, setSavedCities, setPrimaryCityIdState]
  );

  const setPrimaryCity = useCallback(
    (cityId: string) => {
      setPrimaryCityIdState(cityId);
    },
    [setPrimaryCityIdState]
  );

  const clearSearch = useCallback(() => {
    setSearchResults([]);
    setSearchError(null);
  }, []);

  return {
    savedCities,
    primaryCityId,
    searchResults,
    isSearching,
    searchError,
    searchCities,
    addCity,
    removeCity,
    setPrimaryCity,
    clearSearch,
  };
}
