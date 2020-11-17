import React from "react";
import { Link } from "react-router-dom";

const EmpList = () => {
    return(
        <Link to='/employees/add' className='btn btn-primary mt-3 mb-3'>
            Add new employee
        </Link>
    )
}

export default EmpList