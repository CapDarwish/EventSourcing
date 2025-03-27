// Example: src/app/projections/view/page.tsx
'use client'; // This page now fetches data and manages state

import React, { useState, useEffect } from 'react';
import PersonProjectionViewer from '@/components/Projection';

// Interface for a Person (same as used in the viewer)
interface Person {
  id: string;
  name: string;
}

export default function ViewProjectionPage() {
  // State for the list of persons
  const [persons, setPersons] = useState<Person[]>([]);
  const [isFetchingPersons, setIsFetchingPersons] = useState<boolean>(true);
  const [personsFetchError, setPersonsFetchError] = useState<string | null>(
    null,
  );

  const apiUrl = process.env.API_BASE_URL;
  // Effect to fetch persons when the page loads
  useEffect(() => {
    const fetchPersons = async () => {
      setIsFetchingPersons(true);
      setPersonsFetchError(null);
      try {
        // --- !!! Replace with your actual API call to get persons !!! ---
        const response = await fetch(apiUrl+'/api/persons');
        if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);
        const data: Person[] = await response.json();
        setPersons(data);
        // --- End Mock API Call ---
      } catch (err) {
        console.error('Parent: Failed to fetch persons:', err);
        setPersonsFetchError(
          err instanceof Error ? err.message : 'Failed to load person options.',
        );
        setPersons([]);
      } finally {
        setIsFetchingPersons(false);
      }
    };
    fetchPersons();
  }, []); // Run once on mount

  return (
    <main className="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 transition-colors duration-200 ease-in-out">
      {/* Pass the fetched persons data and status down as props */}
      <PersonProjectionViewer
        persons={persons}
        isFetchingPersons={isFetchingPersons}
        personsFetchError={personsFetchError}
      />
    </main>
  );
}

