import React from 'react';
import authService from '../api-authorization/AuthorizeService';
import { Col, Form, FormGroup, Input, Label, Row } from 'reactstrap';
import AddForm from '../base/add-form.component';

class KilometersFileInsert extends AddForm {
    constructor(props) {
        super(props);

        this.state = {
            gpsFile: null,
            odoFile: null,
            successMessage: "Successfully inserted file data.",
            success: false,
            error: false,
            loading: true,
            gpsFiles: [],
            odoFiles: []
        }
        this.addToDatabase = this.addToDatabase.bind(this);
        this.getFilesFromDatabase = this.getFilesFromDatabase.bind(this);
        this.SetStateFromDb = this.SetStateFromDb.bind(this);
    }

    componentDidMount() {
        this.getFilesFromDatabase();
        this.setState({
            loading: false
        });
    }

    getFilesFromDatabase() {
        let token;
        authService.getAccessToken()
            .then(res => {
                token = res;
            })
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
        fetch('api/kilometersfile/', {
            headers,
            method: 'GET'
        })
            .then(res => {
                if(res.ok) {
                    return res.json()
                }
                else {
                    this.setState({
                        loading: false,
                        error: true
                    })
                    return null;
                }
            })
            .then(data => {
                
                    console.log(data);
                    let gpsFiles = [];
                    let odoFiles = [];
                    for (var file of data) {
                        if (!file.IsUploaded)
                            file.IsGps ? gpsFiles.push(file) : odoFiles.push(file);
                    }

                    console.log(odoFiles);

                    this.setState({
                        gpsFiles: gpsFiles,
                        odoFiles: odoFiles
                    })                
            }
            )
            .catch(function (error) {
                this.setState({
                    loading: false,
                    error: true
                });
                return null;
            })
    }

    SetStateFromDb() {
        let data;
        data = this.getFilesFromDatabase()
        console.log(data);
        let gpsFiles = [];
        let odoFiles = [];
        for (let file in data) {
            if (!file.IsUploaded)
                file.IsGps ? gpsFiles.push(file) : odoFiles.push(file);
        }

        console.log(odoFiles);

        this.setState({
            gpsFiles: gpsFiles,
            odoFiles: odoFiles
        })
    }

    onFileChange = (event) => {
        // Update the state 
        this.setState({ [event.target.name]: event.target.files[0] });
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
        fetch('api/kilometersfile', {
            headers: headers,
            method: 'POST',
            body: model
        })
            .then(response => {
                if (!response.ok) {
                    this.setState({
                        loading: false,
                        error: true
                    })
                }
                else {
                    console.log(response.json());
                    this.setState({
                        loading: false,
                        success: true
                    });
                }
            })
            .catch(function (error) {
                console.log(error);
                this.setState({
                    loading: false,
                    error: true
                });
            });

    }

    returnForm = () => {
        const selects = !this.state.loading ?
            <div>
                <FileSelect arr={this.state.gpsFiles} label='Select GPS file' name='gpsFile' />
                <FileSelect arr={this.state.odoFiles} label='Select ODO file' name='odoFile' />
            </div >
            : null;
        return (
            <Row>
                <Col sm={{ size: 6, offset: 3 }}>
                    <h2 id="tabelLabel" > Select files for data aggregation </h2>
                    <p>These files should contain information about kilometer data from GPS module and
                       from car's odometer in the dashboard.</p>
                    <Form onSubmit={this.onSubmit}>
                        {selects}
                        <FormGroup>
                            <Input type='submit' name='submit' value='Add file data to db' />
                        </FormGroup>
                    </Form>
                </Col>
            </Row>
        )
    }
}
const FileSelect = props => {
    if (!Array.isArray(props.arr))
        return null;
    else {
        //console.log(props.name);
        //console.log(props.arr);
        return (
            <FormGroup>
                <Label> {props.label} </Label>
                <Input type='select' name={props.name}>
                    {
                        props.arr.map(item => {
                            return (
                                <option key={item.id} value={item.id}>{item.fileName}</option>
                            )
                        })
                    }
                </Input>
            </FormGroup>
        )
    }
}

export default KilometersFileInsert
