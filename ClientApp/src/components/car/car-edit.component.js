import React, { Component } from 'react';
import { Col, Form, FormGroup, Input, Label, Row, Spinner } from 'reactstrap';
import authService from '../api-authorization/AuthorizeService';

class CarEdit extends Component {
    constructor(props) {
        super(props);

        this.state = {
            numberplate: '',
            carmodel: '',
            make: '',
            loading: false
        }
        this.ReturnCarFromDatabase = this.ReturnCarFromDatabase.bind(this);
        this.SetStateFromDb = this.SetStateFromDb.bind(this);
        this.UpdateDatabase = this.UpdateDatabase.bind(this);
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
        if(!token) {
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
            UpdatedAt: new Date()
        };
        console.log(car);
        try {
            this.UpdateDatabase(car);
        }
        catch(err) {
            console.error(`Error: ${err}`);
        }
    }

    async UpdateDatabase(car) {
       
        try {
            const token = await authService.getAccessToken();
            let headers = {};
            if(!token) {
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
                method: 'PUT',
                body: JSON.stringify(car)
            });
            this.setState({ 
                numberplate: '',
                carmodel: '',
                make: '' 
            });
            this.props.history.push('/cars');
            // window.location = '/cars';
        }
        catch(err) {
            console.log(`Error: ${err}`);
        }
    }
    
    returnForm = () => {
        return (
            <Row>
                <Col sm={{size: 6, offset: 3}}>
                    <h1 id="tabelLabel" > Update car </h1>
                    <Form onSubmit={this.onSubmit}>
                        <FormGroup>
                            <Label>Number plate</Label>
                            <Input name='numberplate' value={this.state.numberplate} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Label>Make</Label>
                            <Input name='make' value={this.state.make} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Label>Model</Label>
                            <Input name='carmodel' value={this.state.carmodel} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Input type='submit' name='submit' value='Update'/> 
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
                { contents }
            </div>
        );
    }
}

export default CarEdit