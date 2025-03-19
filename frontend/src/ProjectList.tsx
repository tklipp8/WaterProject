import { useEffect, useState } from "react";
import { Project } from "./types/Project";


function ProjectList() {

    const[projects, setProjects] = useState<Project[]>([]);

    useEffect(() => {
        const fetchProjects = async () => {
            const response = await fetch('https://localhost:5000/Water/AllProjects');
            const data = await response.json();
            setProjects(data); 
        };

        fetchProjects();
    }, []);


    return (
        <>
            <h1>Water Projects</h1>
            <br />
            {projects.map((p) =>
                <div id='projectCard' className='card'>
                    <h3 className='card-title'>{p.projectName}</h3>
                    <div className='card-body'>
                        <ul className="list-unstyled">
                            <li>
                                <strong>Project Type: </strong> 
                                {p.projectType}
                            </li>
                            <li>
                                <strong>Regional Program: </strong>
                                {p.projectRegionalProgram}
                            </li>
                            <li>
                                <strong>Impact: </strong>
                                {p.projectImpact} Individuals Served
                            </li>
                            <li>
                                <strong>Project Phase: </strong>
                                {p.projectPhase}
                            </li>
                            <li>
                                <strong>Project Status: </strong>
                                {p.projectFunctionalityStatus}
                            </li>
                        </ul>                        
                    </div>


                </div>
        )}
        </>
    );
}

export default ProjectList