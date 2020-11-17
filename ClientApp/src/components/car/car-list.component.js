import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService';
import { Link } from 'react-router-dom';
import { Spinner } from 'reactstrap';

class CarList extends Component {
    constructor(props) {
        super(props);

        this.state = {
            loading: false,
            cars: []
        }
    }

    componentDidMount() {
        this.ReturnDataFromDatabase();
    }

    async ReturnDataFromDatabase() {
        const token = await authService.getAccessToken();
        let headers = {};
        if (!token) {
            headers = {
                'Accept': 'application/json',
                'Content-Type': 'application/json;charset=UTF-8'
            }
        } else {
            headers = {
                'Authorization': `Bearer ${token}`,
                'Accept': 'application/json',
                'Content-Type': 'application/json;charset=UTF-8'
            }
        }
        const response = await fetch('api/car', {
            headers: headers,
            method: 'GET'
        });
        const data = await response.json();
        //console.log(data);
        this.setState({ loading: false, cars: data });
    }

    returnTable() {
        if (Array.isArray(this.state.cars)) {
            return (
                <div>
                    <table className='table'>
                        <thead>
                            <tr>
                                <td>Numberplate</td>
                                <td>Make</td>
                                <td>Model</td>
                                <td>Actions</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                this.state.cars.map(car => {
                                    return <tr key={car.id}>
                                        <td>{car.numberplate}</td>
                                        <td>{car.make}</td>
                                        <td>{car.carmodel}</td>
                                        <td>
                                            <Link to={`/cars/edit/${car.id}`}>Edit</Link> | <Link to={`/cars/remove/${car.id}`}>Remove</Link>
                                        </td>
                                    </tr>;
                                }
                                )
                            }
                        </tbody>
                    </table>
                </div>
            )
        }
        else {
            return (
                <p>Could not load a list.</p>
            )
        }

    }

    render() {

        let contents = this.state.loading
            ? <Spinner size="lg" color="secondary" />
            : this.returnTable();

        return (
            <div>
                <h2 id="tabelLabel" >List of cars</h2>
                <Link to='/cars/add' className='btn btn-primary mt-3 mb-3'>Add a car</Link>
                {contents}
            </div>
        );
    }
}

export default CarList