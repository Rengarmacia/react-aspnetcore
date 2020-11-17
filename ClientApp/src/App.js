import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';

import './custom.css'
import CarAdd from './components/car/car-add.component';
import CarList from './components/car/car-list.component';
import CarEdit from './components/car/car-edit.component';
import CarRemove from './components/car/car-remove.component';

import EmpAdd from "./components/emp/emp-add.component";
import EmpList from './components/emp/emp-list.component';

import KilometersFileUpload from './components/kilometers/kilometers-file-upload.component';
import KilometersFileInsert from './components/kilometers/kilometers-insert.component';

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Switch>
                    {/* Cars content */}
                    <AuthorizeRoute path='/km/upload' exact component={KilometersFileUpload} />
                    <AuthorizeRoute path='/km/insert' exact component={KilometersFileInsert} />
                    <AuthorizeRoute path='/cars/edit/:id' exact component={CarEdit} />
                    <AuthorizeRoute path='/cars/remove/:id' exact component={CarRemove} />
                    <AuthorizeRoute path='/cars/add' exact component={CarAdd} />
                    <AuthorizeRoute path='/cars/' component={CarList} />
                    {/* Employee content */}
                    <AuthorizeRoute path='/employees/add' exact component={EmpAdd} />
                    <AuthorizeRoute path='/employees/' exact component={EmpList} />
                </Switch>

                <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
            </Layout>
        );
    }
}
