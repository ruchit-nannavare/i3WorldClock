import { useState, useRef, useEffect } from 'react';
import { FiSearch, FiPlus, FiCheck } from 'react-icons/fi';
import type { City } from '../types';
import './SearchBar.css';

interface SearchBarProps {
  searchResults: City[];
  isSearching: boolean;
  onSearch: (query: string) => void;
  onAddCity: (city: City) => void;
  onClear: () => void;
  savedCityIds: string[];
}

export function SearchBar({
  searchResults,
  isSearching,
  onSearch,
  onAddCity,
  onClear,
  savedCityIds,
}: SearchBarProps) {
  const [query, setQuery] = useState('');
  const [isFocused, setIsFocused] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(event.target as Node)
      ) {
        setIsFocused(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setQuery(value);
    onSearch(value);
  };

  const handleAddCity = (city: City) => {
    onAddCity(city);
    setQuery('');
    onClear();
    setIsFocused(false);
  };

  const showDropdown = isFocused && (searchResults.length > 0 || isSearching);

  return (
    <div className="search-bar-container" ref={containerRef}>
      <div className={`search-bar-box ${showDropdown ? 'search-bar-box-expanded' : ''}`}>
        <div className={`search-bar-wrapper ${isFocused ? 'search-bar-wrapper-focused' : ''}`}>
          <span className="search-icon">
            <FiSearch size={16} />
          </span>
          <input
            ref={inputRef}
            type="text"
            className="search-bar-input"
            placeholder="Search"
            value={query}
            onChange={handleInputChange}
            onFocus={() => setIsFocused(true)}
          />
        </div>
        <div className={`search-bar-dropdown ${showDropdown ? 'search-bar-dropdown-visible' : ''}`}>
          {isSearching ? (
            <div className="search-bar-loading">Searching...</div>
          ) : (
            searchResults.map((city) => {
              const isAlreadyAdded = savedCityIds.includes(city.id);
              return (
                <div key={city.id} className="search-bar-result">
                  <span className="search-bar-result-name">
                    {city.name}, {city.country}
                  </span>
                  <button
                    className="search-bar-add-btn"
                    onClick={() => handleAddCity(city)}
                    disabled={isAlreadyAdded}
                  >
                    {isAlreadyAdded ? <FiCheck size={14} /> : <FiPlus size={14} />}
                  </button>
                </div>
              );
            })
          )}
        </div>
      </div>
    </div>
  );
}
