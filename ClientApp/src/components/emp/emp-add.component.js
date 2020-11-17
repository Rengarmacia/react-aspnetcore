import React, { Component } from 'react';
import authService from '../api-authorization/AuthorizeService';
import { Form, FormGroup, Label, Input, Row, Col, Spinner, Alert } from 'reactstrap';

class EmpAdd extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fname: '',
            lname: '',
            emplid: '',
            city: '',
            group: '',
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

        const employee = {
            Fname: this.state.fname,
            Lname: this.state.lname,
            EmplId: this.state.emplid,
            City: this.state.city,
            Group: this.state.group
        };
        try {
            await this.addToDatabase(employee);
        }
        catch(err) {
            console.error(`Error: ${err}`);
        }
    }

    async addToDatabase(employee) {
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
        try {
            //const response =
            await fetch('api/employee', {
                headers: headers,
                method: 'POST',
                body: JSON.stringify(employee)
            });
            //const data = await response.json();
        }
        catch(err) {
            this.setState({
                error: true
            })
            console.log(`Error: ${err}`);
        }
        this.setState({ 
            fname: '',
            lname: '',
            emplid: '',
            city: '',
            group: '',
            loading: false,
            success: true
         });
    }

    returnForm() {
        return (
            <Row>
                <Col sm={{size: 6, offset: 3}}>
                    <h1 id="tabelLabel" > Create Employee</h1>
                    <Form onSubmit={this.onSubmit}>
                        <Row>
                            <Col>
                                <FormGroup>
                                    <Label>First Name</Label>
                                    <Input name='fname' value={this.state.fname} onChange={this.onChange}/>
                                </FormGroup>
                            </Col>
                            <Col>
                                <FormGroup>
                                    <Label>Last Name</Label>
                                    <Input name='lname' value={this.state.lname} onChange={this.onChange}/>
                                </FormGroup>
                            </Col>
                        </Row>
                        <FormGroup>
                            <Label>Axapta Employee Id</Label>
                            <Input name='emplid' value={this.state.emplid} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Label>City</Label>
                            <Input name='city' value={this.state.city} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Label>Group</Label>
                            <Input name='group' value={this.state.group} onChange={this.onChange}/>
                        </FormGroup>
                        <FormGroup>
                            <Input type='submit' name='submit' value='Add a new emplyee'/> 
                        </FormGroup>
                    </Form>
                </Col>
            </Row>
        )
    }
    openError = () => {
        this.setState({
            error: true
        })        
    }
    render() {
        let contents = this.state.loading
            ? <Spinner size="lg" color="secondary" />
            : this.returnForm();
        return (
            <div>
                <Alert color='success' isOpen={this.state.success} toggle={this.closeAlert}>
                    Successfuly added a new employee to database!
                </Alert>
                <Alert color='danger' isOpen={this.state.error} toggle={this.closeError}>
                    Something went wrong. Please, try again later.
                </Alert>
                { contents }
                {/* <Button onClick={this.openError}>
                    Click to test
                </Button> */}
            </div>
        );
    }
}

export default EmpAdd