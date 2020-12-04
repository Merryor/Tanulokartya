/**
 * User interface
 */
export interface User {
    id: string;
    name: string;
    phone: string;
    email: string;
    password: string;
    password_again: string;
    workplace: string;
    create_module: number,
    will_create_module: number,
    roles: Array<string>;
    activated: boolean;
    token?: string;
}
