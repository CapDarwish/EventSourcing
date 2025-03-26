// src/components/PersonList.tsx
'use client';

import React, { useState, useEffect } from 'react';

export interface Person { // Export interface for use in parent
  id: string;
  name: string;
}

interface PersonListProps {
  // Function to call when the Edit button is clicked for a person
  onEdit: (person: Person) => void;
  // Key to force re-fetching when it changes
  refreshKey: number;
}

const PersonList: React.FC<PersonListProps> = ({ onEdit, refreshKey }) => {
  const [persons, setPersons] = useState<Person[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const baseUrl = process.env.API_BASE_URL;
  useEffect(() => {
    const fetchPersons = async () => {
      setIsLoading(true);
      setError(null);
      console.log('Fetching persons (refreshKey:', refreshKey, ')'); // Log fetch trigger
      try {
        // --- !!! Replace with your actual API call !!! ---
        const response = await fetch(baseUrl + '/api/persons');
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const data: Person[] = await response.json();

        console.log('Received persons:', data); // Log response
        setPersons(data);
        // --- End Mock API Call ---
      } catch (err) {
        console.error('Failed to fetch persons:', err);
        setError(
          err instanceof Error ? err.message : 'An unknown error occurred.',
        );
        setPersons([]);
      } finally {
        setIsLoading(false);
      }
    };

    fetchPersons();
  }, [refreshKey]); // Re-run effect when refreshKey changes

  const renderContent = () => {
    // ... (Loading and Error states remain the same as before) ...
    if (isLoading) {
      return (
        <p className="text-center text-gray-500 dark:text-gray-400 py-4">
          Loading persons...
        </p>
      );
    }

    if (error) {
      return (
        <div
          className="p-3 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-600 text-red-700 dark:text-red-300 rounded-md text-sm text-center"
          role="alert"
        >
          <p>
            <span className="font-medium">Error:</span> {error}
          </p>
        </div>
      );
    }

    if (persons.length === 0) {
      return (
        <p className="text-center text-gray-500 dark:text-gray-400 py-4">
          No persons found.
        </p>
      );
    }else{

        console.log('persons:', persons);
    }

    return (
      <ul className="divide-y divide-gray-200 dark:divide-gray-700">
        {persons.map((person) => (
          <li
            key={person.id}
            className="py-3 px-1 flex justify-between items-center space-x-3"
          >
            <span className="text-gray-800 dark:text-gray-200 truncate">
              {person.name}
            </span>
            <button
              onClick={() => onEdit(person)}
              className="flex-shrink-0 px-3 py-1 border border-indigo-500 dark:border-indigo-400 text-indigo-600 dark:text-indigo-300 rounded-md text-xs font-medium hover:bg-indigo-50 dark:hover:bg-indigo-900/30 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:focus:ring-offset-gray-800 transition duration-150 ease-in-out"
            >
              Edit
            </button>
          </li>
        ))}
      </ul>
    );
  };

  return (
    <div className="max-w-md mx-auto mt-8 p-6 bg-white dark:bg-gray-800 rounded-lg shadow-xl dark:shadow-2xl border border-gray-200 dark:border-gray-700">
      <h2 className="text-xl font-semibold text-gray-800 dark:text-gray-100 mb-4 text-center">
        Person List
      </h2>
      {renderContent()}
    </div>
  );
};

export default PersonList;


