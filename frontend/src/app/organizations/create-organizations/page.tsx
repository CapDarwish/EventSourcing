// Example: src/app/organizations/create/page.tsx
'use client';

import React from 'react';
import CreateOrganizationForm from '@/components/CreateOrganizationForm';
// Possibly import useRouter if you want to redirect after creation
// import { useRouter } from 'next/navigation';

export default function CreateOrganizationPage() {
  // const router = useRouter();

  const handleCreationComplete = () => {
    console.log('Organization creation complete!');
    // Example: Refresh data or navigate elsewhere
    // router.push('/organizations'); // Navigate to the list page
    // Or trigger a refresh of a list on the *same* page if applicable
    alert('Organization Unit Created Successfully!'); // Simple feedback
  };

  return (
    <main className="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 transition-colors duration-200 ease-in-out">
      <CreateOrganizationForm onCreateComplete={handleCreationComplete} />
      {/* You might also have a list of organizations displayed elsewhere */}
    </main>
  );
}

