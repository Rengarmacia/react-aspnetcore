import React from 'react';
import authService from '../api-authorization/AuthorizeService';
import { Col, Form, FormGroup, Input, Label, Row } from 'reactstrap';
import AddForm from '../base/add-form.component';

class KilometersFileUpload extends AddForm {
    constructor(props) {
        super(props);

        this.state = {
            file: undefined,
            GPS: false,
            successMessage: "Successfully Uploaded a file.",
            success: false,
            error: false
        }
        this.addToDatabase = this.addToDatabase.bind(this);
    }

    onFileChange = (event) => {
        // Update the state 
        this.setState({ file: event.target.files[0] });
    }; 

    getDataModel() {
        let formData = new FormData();
        formData.append("File", this.state.file);
        formData.append("Conditional", this.state.GPS);
        formData.append("UploadedFileName", "");
        return formData;
    }

    async addToDatabase(model) {
        const token = await authService.getAccessToken();
        let headers = {};
        if (!token) {
            headers = {
                //'Accept': 'application/json',
                //'Content-Type': 'application/json;charset=UTF-8'
            }
        } else {
            headers = {
                'Authorization': `Bearer ${token}`,
                //'Accept': 'application/json',
                //'Content-Type': 'application/json;charset=UTF-8'
            }
        }
        try {
            const response = await fetch('api/kilometersfile', {
                headers: headers,
                method: 'POST',
                body: model
            });
            if (!response.ok) {
                this.setState({
                    loading: false,
                    error: true
                });
            }
            else {
                console.log(response.json());
                this.setState({
                    loading: false,
                    success: true
                });
            }
        }
        catch (err) {
            this.setState({
                loading: false,
                error: true
            });
            console.log(`Error: ${err}`);
        }
        
    }

    returnForm() {
        return (
            <Row>
                <Col sm={{ size: 6, offset: 3 }}>
                    <h2 id="tabelLabel" > Upload an Excel file </h2>
                    <p>This file should contain information about kilometer data from GPS module, or
                       from car's odometer in the dashboard</p>
                    <Form onSubmit={this.onSubmit}>
                        <FormGroup>
                            <Label>Select a file</Label>
                            <Input
                                name='file'
                                onChange={this.onFileChange}
                                type='file'
                                accept='.xlsx,.xls'
                                required />
                        </FormGroup>
                        <FormGroup check>
                            <Label for='gps_cb' check>
                                <Input id='gps_cb' type='checkbox' name='GPS' value={this.state.GPS} onChange={this.onChange} />
                                Is it a GPS file?
                            </Label>
                        </FormGroup>
                        <FormGroup>
                            <Input type='submit' name='submit' value='Upload the file' />
                        </FormGroup>
                    </Form>
                </Col>
            </Row>
        )
    }
}

export default KilometersFileUpload
