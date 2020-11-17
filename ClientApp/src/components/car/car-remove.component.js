import React, { Component } from 'react';
import { Col, Form, FormGroup, Input, Label, Row, Spinner, Alert } from 'reactstrap';
import authService from '../api-authorization/AuthorizeService';
import { Link } from 'react-router-dom';

class CarRemove extends Component {
    constructor(props) {
        super(props);

        this.state = {
            numberplate: '',
            carmodel: '',
            make: '',
            loading: false,
            error: false,
            success: false,
            deleted: false
        }
        this.ReturnCarFromDatabase = this.ReturnCarFromDatabase.bind(this);
        this.SetStateFromDb = this.SetStateFromDb.bind(this);
        this.DeleteFromDatabase = this.DeleteFromDatabase.bind(this);
    }

    componentDidMount() {
        this.SetStateFromDb();
    }

    async SetStateFromDb() {
        const data = await this.ReturnCarFromDatabase();
        this.setState({
            numberplate: data.numberplate == null ? '' : data.numberplate,
            carmodel: data.carmodel == null ? '' : data.carmodel,
            make: data.make == null ? '' : data.make
        })
    }

    async ReturnCarFromDatabase() {
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
        const response = await fetch('api/car/' + this.props.match.params.id, {
            headers,
            method: 'GET'
        });
        const data = await response.json();
        console.log(data);
        this.setState({ loading: false });
        return data;
    }

    onChange = (e) => {
        const target = e.target.name;
        const value = e.target.value;
        this.setState({
            [target]: value
        })
    }

    onSubmit = (e) => {
        //prevents default form submition behaviour
        e.preventDefault();

        this.setState({
            loading: true
        });

        const car = {
            Id: this.props.match.params.id,
            Numberplate: this.state.numberplate,
            Carmodel: this.state.carmodel,
            Make: this.state.make,
        };
        //console.log(car);
        try {
            this.DeleteFromDatabase(car);
        }
        catch (err) {
            console.error(`Error: ${err}`);
        }
    }

    async DeleteFromDatabase(car) {

        try {
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
            await fetch('api/car/' + this.props.match.params.id, {
                //headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                headers: headers,
                method: 'DELETE',
                body: JSON.stringify(car)
            });
            this.setState({
                numberplate: '',
                carmodel: '',
                make: '',
                success: true,
                loading: false, 
                deleted: true
            });
            //this.props.history.push('/cars');
            // window.location = '/cars';
        }
        catch (err) {
            this.setState({
                loading: false,
                error: true
            });
            console.log(`Error: ${err}`);
        }
    }

    returnForm = () => {
        return (
            <Row>
                <Col sm={{ size: 6, offset: 3 }}>
                    <h1 id="tabelLabel" > Delete car </h1>
                    <Form onSubmit={this.onSubmit}>
                        <FormGroup>
                            <Label>Number plate</Label>
                            <Input name='numberplate' value={this.state.numberplate} onChange={this.onChange} disabled/>
                        </FormGroup>
                        <FormGroup>
                            <Label>Make</Label>
                            <Input name='make' value={this.state.make} onChange={this.onChange} disabled/>
                        </FormGroup>
                        <FormGroup>
                            <Label>Model</Label>
                            <Input name='carmodel' value={this.state.carmodel} onChange={this.onChange} disabled/>
                        </FormGroup>
                        <FormGroup>
                            <Label> Are you sure you want to delete this? </Label>
                            <Input type='submit' name='submit' value='Delete' />
                        </FormGroup>
                    </Form>
                </Col>
            </Row>
        )
    }

    closeAlert = () => {
        this.setState({
            success: false
        })
    }

    closeError = () => {
        this.setState({
            error: false
        })
    }

    render() {

        let contents = null;
        if (this.state.loading) {
            contents = <Spinner size="lg" color="secondary" />;
        }
        else if (!this.state.deleted) {
            contents = this.returnForm();
        }
 
        return (
            <div>
                <Alert color='success' isOpen={this.state.success} toggle={this.closeAlert}>
                    Successfully removed the car!
                </Alert>
                <Alert color='danger' isOpen={this.state.error} toggle={this.closeError}>
                    Something went wrong. Please, try again later.
                </Alert>
                { contents }
                <Link to='/cars/'>Go back</Link>
            </div>
        );
    }
}

export default CarRemove