import React from 'react';
import { Component } from 'react';
import { Spinner, Alert } from 'reactstrap';
// for this class to be implemented
// you need to have:
// async addToDatabase(data) method
// returnForm method
// this.state = {
//     success: false,
//     error: false,
//     successMessage: "<your message>"
// }
// getDataModel method
export default class AddForm extends Component {
    constructor(props) {
        super(props);
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

        const data = this.getDataModel();

        try {
            await this.addToDatabase(data);
        }
        catch (err) {
            console.error(`Error: ${err}`);
        }
    }

    render() {

        let contents = this.state.loading
            ? <Spinner size="lg" color="secondary" />
            : this.returnForm();

        return (
            <div>
                <Alert color='success' isOpen={this.state.success} toggle={this.closeAlert}>
                    {this.state.successMessage}
                </Alert>
                <Alert color='danger' isOpen={this.state.error} toggle={this.closeError}>
                    Something went wrong. Please, try again later.
                </Alert>
                {contents}
            </div>
        );
    }
}