import CategoryFilter from '../components/CategoryFilter';
import { Fingerprint } from '../Fingerprint';
import ProjectList from '../components/ProjectList';
import CookieConsent from "react-cookie-consent";
import WelcomeBand from '../components/WelcomBand';
import { useState } from 'react';
import CartSummary from '../components/CartSummary';

function ProjectPage () {
    const [selectedCategories, setSelectedCategories] = useState<string[]>([]);

    return (
        <>
      <div className='container mt-4'>
        <CartSummary />
        <WelcomeBand />
        <div className='row'>
          <div className='col-md-3'>
            <CategoryFilter
              selectedCategories={selectedCategories}
              setSelectedCategories={setSelectedCategories}
            />
          </div>
          <div className='col-md-9'>
            <ProjectList selectedCategories={selectedCategories}/>
          </div>
        </div>
      </div>
      <CookieConsent>
        This website uses cookies to enhance the user experience.
      </CookieConsent>
      <Fingerprint />        
        </>

    );
}

export default ProjectPage