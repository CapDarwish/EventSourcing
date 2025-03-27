// src/components/PersonProjectionViewer.tsx
'use client';

import React, { useState, FormEvent } from 'react';

// Interface for the expected API response data
interface PersonProjection {
  id: string;
  name: string;
  units: string[];
}

// Interface for a Person (to be passed as props)
interface Person {
  id: string;
  name: string;
}

// Props for the component
interface PersonProjectionViewerProps {
  persons: Person[]; // List of persons to populate the dropdown
  isFetchingPersons: boolean; // Is the parent fetching persons?
  personsFetchError: string | null; // Any error from fetching persons?
}

const PersonProjectionViewer: React.FC<PersonProjectionViewerProps> = ({
  persons,
  isFetchingPersons,
  personsFetchError,
}) => {
  // State for selected person ID and input version
  const [selectedPersonId, setSelectedPersonId] = useState<string>(''); // Store selected ID
  const [inputVersion, setInputVersion] = useState<string>('');

  // State for API response/status
  const [projectionData, setProjectionData] = useState<PersonProjection | null>(
    null,
  );
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const baseUrl = process.env.API_BASE_URL;
  const handleFetchProjection = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setProjectionData(null);

    // Validation
    if (!selectedPersonId) {
      setError('Please select a person.');
      return;
    }
    if (!/^\d+$/.test(inputVersion)) {
      setError('Version must be a valid non-negative number.');
      return;
    }

    setIsLoading(true);

    const apiUrl = `/api/persons/projection/${encodeURIComponent(
      selectedPersonId, // Use selected ID
    )}/${encodeURIComponent(inputVersion)}`;

    console.log(`Fetching projection from: ${apiUrl}`);

    try {
      // --- !!! Make the actual API call !!! ---
      const response = await fetch(baseUrl + apiUrl);
      if (!response.ok) {
        let errorMessage = `Error: ${response.status} ${response.statusText}`;
        try {
          const errorBody = await response.json();
          errorMessage =
            errorBody.message || errorBody.title || errorMessage;
        } catch (e) {}
        throw new Error(errorMessage);
      }
      const data: PersonProjection = await response.json();
      console.log('API Fetch Projection Success:', data);
      setProjectionData(data);
      // --- End API call ---
    } catch (err) {
      console.error('Failed to fetch person projection:', err);
      setError(
        err instanceof Error ? err.message : 'An unknown error occurred.',
      );
      setProjectionData(null);
    } finally {
      setIsLoading(false);
    }
  };

  // Determine if the main submit button should be disabled
  const isSubmitDisabled =
    isLoading || isFetchingPersons || !selectedPersonId || !inputVersion;

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white dark:bg-gray-800 rounded-lg shadow-xl dark:shadow-2xl border border-gray-200 dark:border-gray-700">
      <h2 className="text-2xl font-semibold text-gray-800 dark:text-gray-100 mb-6 text-center">
        View Person Projection
      </h2>

      {/* Input Form */}
      <form onSubmit={handleFetchProjection} noValidate className="space-y-4">
        {/* Person Select Dropdown */}
        <div>
          <label
            htmlFor="personSelect"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Person <span className="text-red-500">*</span>
          </label>
          <select
            id="personSelect"
            value={selectedPersonId}
            onChange={(e) => {
              setSelectedPersonId(e.target.value);
              if (error) setError(null);
              if (projectionData) setProjectionData(null);
            }}
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading || isFetchingPersons} // Disable while loading projection or persons list
          >
            <option value="" disabled={isFetchingPersons}>
              {isFetchingPersons ? 'Loading persons...' : '-- Select Person --'}
            </option>
            {!isFetchingPersons &&
              persons.map((person) => (
                <option key={person.id} value={person.id}>
                  {person.name}
                </option>
              ))}
          </select>
          {/* Display error if fetching persons failed */}
          {personsFetchError && !isFetchingPersons && (
            <p className="mt-1 text-xs text-red-600 dark:text-red-400">
              Error loading persons: {personsFetchError}
            </p>
          )}
        </div>

        {/* Version Input */}
        <div>
          <label
            htmlFor="versionInput"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Version (long) <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            inputMode="numeric"
            pattern="[0-9]*"
            id="versionInput"
            value={inputVersion}
            onChange={(e) => {
              setInputVersion(e.target.value);
              if (error) setError(null);
              if (projectionData) setProjectionData(null);
            }}
            placeholder="Enter version number (e.g., 0, 1, 2...)"
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600"
            disabled={isLoading || isFetchingPersons} // Also disable if persons are loading
          />
        </div>

        {/* Submit Button */}
        <div className="pt-2">
          <button
            type="submit"
            className={`w-full py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white focus:outline-none focus:ring-2 focus:ring-offset-2 dark:focus:ring-offset-gray-800 transition duration-150 ease-in-out ${
              isSubmitDisabled // Use combined disabled state
                ? 'bg-indigo-400 dark:bg-indigo-700/60 cursor-not-allowed'
                : 'bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600 focus:ring-indigo-500 dark:focus:ring-indigo-400'
            }`}
            disabled={isSubmitDisabled}
          >
            {isLoading ? 'Fetching...' : 'Get Projection'}
          </button>
        </div>
      </form>

      {/* Results Area (remains the same) */}
      <div className="mt-6 min-h-[100px]">
        {isLoading && (
          <p className="text-center text-gray-500 dark:text-gray-400 py-4">
            Loading projection...
          </p>
        )}
        {error && !isLoading && (
          <div
            className="p-3 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-600 text-red-700 dark:text-red-300 rounded-md text-sm"
            role="alert"
          >
            <p>
              <span className="font-medium">Error:</span> {error}
            </p>
          </div>
        )}
        {projectionData && !isLoading && !error && (
          <div className="space-y-3 p-4 border border-gray-200 dark:border-gray-600 rounded-md bg-gray-50 dark:bg-gray-700/50">
            <h3 className="text-lg font-medium text-gray-900 dark:text-gray-100">
              Projection Result:
            </h3>
            <p className="text-sm text-gray-700 dark:text-gray-300">
              <span className="font-medium">ID:</span> {projectionData.id}
            </p>
            <p className="text-sm text-gray-700 dark:text-gray-300">
              <span className="font-medium">Name:</span> {projectionData.name}
            </p>
            <div>
              <p className="text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                Units ({projectionData.units.length}):
              </p>
              {projectionData.units.length > 0 ? (
                <ul className="list-disc list-inside pl-2 space-y-1">
                  {projectionData.units.map((unitId) => (
                    <li
                      key={unitId}
                      className="text-xs text-gray-600 dark:text-gray-400 font-mono"
                    >
                      {unitId}
                    </li>
                  ))}
                </ul>
              ) : (
                <p className="text-sm text-gray-500 dark:text-gray-400 italic">
                  No associated units found for this projection.
                </p>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default PersonProjectionViewer;

