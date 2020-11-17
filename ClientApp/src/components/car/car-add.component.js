import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService';
import { Form, FormGroup, Label, Input, Row, Col, Spinner, Alert } from 'reactstrap';

class CarAdd extends Component {
    constructor(props) {
        super(props);

        this.state = {
            numberplate: '',
            carmodel: '',
            make: '',
            loading: false,
            success: false,
            error: false
        }
        this.addToDatabase = this.addToDatabase.bind(this);
        this.onSubmit = this.onSubmit.bind(this);
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

    onChange = (e) => {
        const target = e.target.name;
        const value = e.target.value;
        this.setState({
            [target]: value
        })
    }

    async onSubmit(e) {
        //prevents default form submition behaviour
        e.preventDefault();

        this.setState({
            loading: true
        });

        const car = {
            Numberplate: this.state.numberplate,
            Carmodel: this.state.carmodel,
            Make: this.state.make
        };
        console.log(car);
        try {
            await this.addToDatabase(car);
        }
        catch (err) {
            console.error(`Error: ${err}`);
        }
    }

    async addToDatabase(car) {
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
        try {
            const response = await fetch('api/car', {
                headers: headers,
                method: 'POST',
                body: JSON.stringify(car)
            });
            const data = await response.json();
            console.log(data);
        }
        catch (err) {
            this.setState({
                error: true
            })
            console.log(`Error: ${err}`);
        }
        this.setState({
            loading: false,
            numberplate: '',
            carmodel: '',
            make: '',
            success: true
        });
    }

    returnForm() {
        return (
            <Row>
                <Col sm={{ size: 6, offset: 3 }}>
                    <h1 id="tabelLabel" > Create car </h1>
                    <Form onSubmit={this.onSubmit}>
                        <FormGroup>
                            <Label>Number plate</Label>
                            <Input
                                name='numberplate'
                                value={this.state.numberplate}
                                onChange={this.onChange}
                                minLength={3}
                                required />
                        </FormGroup>
                        <FormGroup>
                            <Label>Make</Label>
                            <Input name='make' value={this.state.make} onChange={this.onChange} />
                        </FormGroup>
                        <FormGroup>
                            <Label>Model</Label>
                            <Input name='carmodel' value={this.state.carmodel} onChange={this.onChange} />
                        </FormGroup>
                        <FormGroup>
                            <Input type='submit' name='submit' value='Create new car' />
                        </FormGroup>
                    </Form>
                </Col>
            </Row>
        )
    }

    render() {

        let contents = this.state.loading
            ? <Spinner size="lg" color="secondary" />
            : this.returnForm();

        return (
            <div>
                <Alert color='success' isOpen={this.state.success} toggle={this.closeAlert}>
                    Successfully added a new car to database!
                </Alert>
                <Alert color='danger' isOpen={this.state.error} toggle={this.closeError}>
                    Something went wrong. Please, try again later.
                </Alert>
                {contents}
            </div>
        );
    }
}

export default CarAdd