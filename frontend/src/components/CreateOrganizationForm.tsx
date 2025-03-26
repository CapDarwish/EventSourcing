// src/components/CreateOrganizationForm.tsx
'use client';

import React, { useState, useEffect, FormEvent } from 'react';

// Interface for the data needed to create an org unit
interface CreateOrganizationUnitCommand {
  Id: string; // Guid
  Name: string;
  ParentId: string | null; // Guid? (nullable Guid)
}

// Interface for existing org units (for the parent dropdown)
interface OrganizationUnit {
  Id: string; // Guid
  Name: string;
}

interface CreateOrganizationFormProps {
  // Callback function after successful creation
  onCreateComplete: () => void;
}

const CreateOrganizationForm: React.FC<CreateOrganizationFormProps> = ({
  onCreateComplete,
}) => {
  const [name, setName] = useState<string>('');
  const [parentId, setParentId] = useState<string | null>(null); // Store ParentId as string or null
  const [potentialParents, setPotentialParents] = useState<OrganizationUnit[]>(
    [],
  );
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [isFetchingParents, setIsFetchingParents] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [parentFetchError, setParentFetchError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const apiUrl = process.env.API_BASE_URL + '/api/OrganizationUnits';
  // Effect to fetch potential parent organizations on mount
  useEffect(() => {
    const fetchPotentialParents = async () => {
      setIsFetchingParents(true);
      setParentFetchError(null);
      try {
        // --- !!! Replace with your actual API call to get organizations !!! ---
        const response = await fetch(apiUrl); // Your GET endpoint for orgs
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data: OrganizationUnit[] = await response.json();
        setPotentialParents(data);

        // --- Mock API Call ---
        // await new Promise((resolve) => setTimeout(resolve, 900)); // Simulate delay
        // const mockParents: OrganizationUnit[] = [
        //   { Id: 'org-guid-1', Name: 'Headquarters' },
        //   { Id: 'org-guid-2', Name: 'Regional Office Alpha' },
        //   { Id: 'org-guid-3', Name: 'Department Beta' },
        // ];
        // setPotentialParents(mockParents);
        // console.log('Mock fetch parents successful:', mockParents);
        // --- End Mock API Call ---

      } catch (err) {
        console.error('Failed to fetch potential parents:', err);
        setParentFetchError(
          err instanceof Error ? err.message : 'Failed to load parent options.',
        );
        setPotentialParents([]); // Clear options on error
      } finally {
        setIsFetchingParents(false);
      }
    };

    fetchPotentialParents();
  }, []); // Run once on mount

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsLoading(true);
    setError(null);
    setSuccessMessage(null);

    if (!name.trim()) {
      setError('Organization Name cannot be empty.');
      setIsLoading(false);
      return;
    }

    const newId = crypto.randomUUID(); // Generate Guid client-side

    const orgData: CreateOrganizationUnitCommand = {
      Id: newId,
      Name: name.trim(),
      ParentId: parentId, // Use the state value (string or null)
    };

    console.log('Submitting Organization Unit data:', orgData);

    try {
      // --- !!! Replace with your actual POST API call to create org !!! ---
      const response = await fetch(apiUrl, { // Your POST endpoint
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(orgData),
      });

      if (!response.ok) {
        let errorMessage = `HTTP error! status: ${response.status}`;
        try {
            const errorBody = await response.json();
            errorMessage = errorBody.message || errorBody.title || errorMessage;
        } catch (e) {}
        throw new Error(errorMessage);
      }
      console.log('API Create Organization Success');
      // --- Mock API Call ---
      // await new Promise((resolve) => setTimeout(resolve, 1000));
      // console.log('Mock API Create Organization successful!');
      // // --- End Mock API Call ---
      //
      setSuccessMessage(
        `Organization Unit "${orgData.Name}" created successfully!`,
      );
      setName(''); // Clear form
      setParentId(null); // Reset parent selection
      onCreateComplete(); // Notify parent component
    } catch (err) {
      console.error('Failed to create organization unit:', err);
      setError(
        err instanceof Error ? err.message : 'An unknown error occurred.',
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white dark:bg-gray-800 rounded-lg shadow-xl dark:shadow-2xl border border-gray-200 dark:border-gray-700">
      <h2 className="text-2xl font-semibold text-gray-800 dark:text-gray-100 mb-6 text-center">
        Create Organization Unit
      </h2>
      <form onSubmit={handleSubmit} noValidate>
        {/* Name Input */}
        <div className="mb-4">
          <label
            htmlFor="orgName"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Organization Name <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            id="orgName"
            value={name}
            onChange={(e) => {
              setName(e.target.value);
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            placeholder="Enter organization name"
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading}
          />
        </div>

        {/* Parent Organization Select */}
        <div className="mb-4">
          <label
            htmlFor="parentId"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Parent Organization (Optional)
          </label>
          <select
            id="parentId"
            value={parentId ?? ''} // Use empty string for the "No Parent" option value
            onChange={(e) => {
              setParentId(e.target.value === '' ? null : e.target.value); // Set null if empty string selected
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading || isFetchingParents} // Disable while loading form or parents
          >
            <option value="">-- No Parent --</option>
            {isFetchingParents && <option disabled>Loading parents...</option>}
            {!isFetchingParents &&
              potentialParents.map((org) => (
                <option key={org.Id} value={org.Id}>
                  {org.Name}
                </option>
              ))}
          </select>
          {parentFetchError && (
            <p className="mt-1 text-xs text-red-600 dark:text-red-400">
              Error loading parents: {parentFetchError}
            </p>
          )}
        </div>

        {/* Error Message */}
        {error && (
          <div
            className="mb-4 p-3 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-600 text-red-700 dark:text-red-300 rounded-md text-sm"
            role="alert"
          >
            <p>
              <span className="font-medium">Error:</span> {error}
            </p>
          </div>
        )}

        {/* Success Message */}
        {successMessage && (
          <div
            className="mb-4 p-3 bg-green-100 dark:bg-green-900/30 border border-green-400 dark:border-green-600 text-green-700 dark:text-green-300 rounded-md text-sm"
            role="alert"
          >
            <p>
              <span className="font-medium">Success:</span> {successMessage}
            </p>
          </div>
        )}

        {/* Submit Button */}
        <div className="mt-6">
          <button
            type="submit"
            className={`w-full py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white focus:outline-none focus:ring-2 focus:ring-offset-2 dark:focus:ring-offset-gray-800 transition duration-150 ease-in-out ${
              isLoading || isFetchingParents // Also disable button while fetching parents
                ? 'bg-indigo-400 dark:bg-indigo-700/60 cursor-not-allowed'
                : 'bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600 focus:ring-indigo-500 dark:focus:ring-indigo-400'
            }`}
            disabled={isLoading || isFetchingParents}
          >
            {isLoading ? 'Creating...' : 'Create Organization Unit'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateOrganizationForm;

