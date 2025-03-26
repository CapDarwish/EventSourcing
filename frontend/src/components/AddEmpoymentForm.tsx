// src/components/AddEmploymentForm.tsx
'use client';

import React, { useState, useEffect, FormEvent } from 'react';

// Interfaces for data fetching
interface Person {
  id: string;
  name: string;
}

interface OrganizationUnit {
  id: string;
  name: string;
}

// Interface for the command payload
interface AddEmploymentCommand {
  personId: string; // Guid
  organizationUnitId: string; // Guid
  role: string;
}

interface AddEmploymentFormProps {
  // Callback function after successful association
  onAssociationComplete: () => void;
}

const AddEmploymentForm: React.FC<AddEmploymentFormProps> = ({
  onAssociationComplete,
}) => {
  // Form State
  const [selectedPersonId, setSelectedPersonId] = useState<string>('');
  const [selectedOrgId, setSelectedOrgId] = useState<string>('');
  const [role, setRole] = useState<string>('');

  // Data Fetching State
  const [persons, setPersons] = useState<Person[]>([]);
  const [organizations, setOrganizations] = useState<OrganizationUnit[]>([]);
  const [isFetchingPersons, setIsFetchingPersons] = useState<boolean>(true);
  const [isFetchingOrgs, setIsFetchingOrgs] = useState<boolean>(true);
  const [personsFetchError, setPersonsFetchError] = useState<string | null>(
    null,
  );
  const [orgsFetchError, setOrgsFetchError] = useState<string | null>(null);

  // Submission State
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const apiUrl = process.env.API_BASE_URL;
  // Fetch Persons
  useEffect(() => {
    const fetchPersons = async () => {
      setIsFetchingPersons(true);
      setPersonsFetchError(null);
      try {
        // --- !!! Replace with your actual API call to get persons !!! ---
        const response = await fetch(apiUrl + "/api/persons");
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data: Person[] = await response.json();
        setPersons(data);

      } catch (err) {
        console.error('Failed to fetch persons:', err);
        setPersonsFetchError(
          err instanceof Error ? err.message : 'Failed to load person options.',
        );
        setPersons([]);
      } finally {
        setIsFetchingPersons(false);
      }
    };
    fetchPersons();
  }, []);

  // Fetch Organizations
  useEffect(() => {
    const fetchOrgs = async () => {
      setIsFetchingOrgs(true);
      setOrgsFetchError(null);
      try {
        // --- !!! Replace with your actual API call to get organizations !!! ---
        const response = await fetch(apiUrl + '/api/organizationunits');
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data: OrganizationUnit[] = await response.json();
        setOrganizations(data);
        // --- End Mock API Call ---
      } catch (err) {
        console.error('Failed to fetch organizations:', err);
        setOrgsFetchError(
          err instanceof Error
            ? err.message
            : 'Failed to load organization options.',
        );
        setOrganizations([]);
      } finally {
        setIsFetchingOrgs(false);
      }
    };
    fetchOrgs();
  }, []);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError(null);
    setSuccessMessage(null);

    // Validation
    if (!selectedPersonId) {
      setError('Please select a person.');
      return;
    }
    if (!selectedOrgId) {
      setError('Please select an organization.');
      return;
    }
    if (!role.trim()) {
      setError('Role cannot be empty.');
      return;
    }

    setIsLoading(true);

    const employmentData: AddEmploymentCommand = {
      personId: selectedPersonId,
      organizationUnitId: selectedOrgId,
      role: role.trim(),
    };

    console.log('Submitting Employment data:', employmentData);

    try {
      // --- !!! Replace with your actual POST API call for employment !!! ---
      const response = await fetch(apiUrl + '/api/persons/' + selectedPersonId +'/employment', { // Your POST endpoint
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(employmentData),
      });

      if (!response.ok) {
        let errorMessage = `HTTP error! status: ${response.status}`;
        try {
            const errorBody = await response.json();
            errorMessage = errorBody.message || errorBody.title || errorMessage;
        } catch (e) {}
        throw new Error(errorMessage);
      }
      console.log('API Add Employment Success');
      setSuccessMessage(
        `Successfully associated person with organization in role "${employmentData.role}".`,
      );
      // Clear form on success
      setSelectedPersonId('');
      setSelectedOrgId('');
      setRole('');
      onAssociationComplete(); // Notify parent
    } catch (err) {
      console.error('Failed to add employment:', err);
      setError(
        err instanceof Error ? err.message : 'An unknown error occurred.',
      );
    } finally {
      setIsLoading(false);
    }
  };

  // Determine if any data is still loading for dropdowns
  const isDataLoading = isFetchingPersons || isFetchingOrgs;

  return (
    <div className="max-w-md mx-auto mt-10 p-6 bg-white dark:bg-gray-800 rounded-lg shadow-xl dark:shadow-2xl border border-gray-200 dark:border-gray-700">
      <h2 className="text-2xl font-semibold text-gray-800 dark:text-gray-100 mb-6 text-center">
        Assign Role (Employment)
      </h2>
      <form onSubmit={handleSubmit} noValidate>
        {/* Person Select */}
        <div className="mb-4">
          <label
            htmlFor="personId"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Person <span className="text-red-500">*</span>
          </label>
          <select
            id="personId"
            value={selectedPersonId}
            onChange={(e) => {
              setSelectedPersonId(e.target.value);
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading || isFetchingPersons}
          >
            <option value="" disabled={isFetchingPersons}>
              {isFetchingPersons ? 'Loading...' : '-- Select Person --'}
            </option>
            {!isFetchingPersons &&
              persons.map((person) => (
                <option key={person.id} value={person.id}>
                  {person.name}
                </option>
              ))}
          </select>
          {personsFetchError && (
            <p className="mt-1 text-xs text-red-600 dark:text-red-400">
              Error loading persons: {personsFetchError}
            </p>
          )}
        </div>

        {/* Organization Select */}
        <div className="mb-4">
          <label
            htmlFor="organizationUnitId"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Organization Unit <span className="text-red-500">*</span>
          </label>
          <select
            id="organizationUnitId"
            value={selectedOrgId}
            onChange={(e) => {
              setSelectedOrgId(e.target.value);
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading || isFetchingOrgs}
          >
            <option value="" disabled={isFetchingOrgs}>
              {isFetchingOrgs ? 'Loading...' : '-- Select Organization --'}
            </option>
            {!isFetchingOrgs &&
              organizations.map((org) => (
                <option key={org.id} value={org.id}>
                  {org.name}
                </option>
              ))}
          </select>
          {orgsFetchError && (
            <p className="mt-1 text-xs text-red-600 dark:text-red-400">
              Error loading organizations: {orgsFetchError}
            </p>
          )}
        </div>

        {/* Role Input */}
        <div className="mb-4">
          <label
            htmlFor="role"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Role <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            id="role"
            value={role}
            onChange={(e) => {
              setRole(e.target.value);
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            placeholder="Enter role (e.g., Manager, Developer)"
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading || isDataLoading} // Disable if fetching dropdown data too
          />
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
              isLoading || isDataLoading // Disable button if submitting OR fetching dropdown data
                ? 'bg-indigo-400 dark:bg-indigo-700/60 cursor-not-allowed'
                : 'bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600 focus:ring-indigo-500 dark:focus:ring-indigo-400'
            }`}
            disabled={isLoading || isDataLoading}
          >
            {isLoading ? 'Assigning...' : 'Assign Role'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default AddEmploymentForm;

